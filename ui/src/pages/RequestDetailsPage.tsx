import { Alert, Button, Divider, Group, List, Paper, Stack, Text, TextInput, Title } from '@mantine/core'
import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { requestApprovalApi } from '../api/requestApprovalApi'
import { ErrorAlert } from '../components/ErrorAlert'
import { LoadingView } from '../components/LoadingView'
import { RejectRequestModal } from '../components/RejectRequestModal'
import { RequestStatusBadge } from '../components/RequestStatusBadge'
import type { RequestDetail } from '../types/request'

export function RequestDetailsPage() {
  const { id = '' } = useParams()
  const [request, setRequest] = useState<RequestDetail>()
  const [changedBy, setChangedBy] = useState('')
  const [error, setError] = useState<string>()
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [rejectOpened, setRejectOpened] = useState(false)

  useEffect(() => {
    requestApprovalApi.get(id)
      .then(setRequest)
      .catch((cause: Error) => setError(cause.message))
      .finally(() => setLoading(false))
  }, [id])

  async function approve() {
    setSaving(true)
    setError(undefined)
    try {
      setRequest(await requestApprovalApi.approve(id, changedBy))
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'Unable to approve request.')
    } finally {
      setSaving(false)
    }
  }

  async function reject(actor: string, reason: string) {
    setSaving(true)
    setError(undefined)
    try {
      setRequest(await requestApprovalApi.reject(id, actor, reason))
      setRejectOpened(false)
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'Unable to reject request.')
    } finally {
      setSaving(false)
    }
  }

  if (loading) return <LoadingView />
  if (!request) return <ErrorAlert message={error ?? 'Request was not found.'} />

  return (
    <Stack>
      <Group justify="space-between">
        <div>
          <Button component={Link} to="/" variant="subtle" px={0}>Back to requests</Button>
          <Title order={2}>{request.title}</Title>
        </div>
        <RequestStatusBadge status={request.status} />
      </Group>
      {error && <Alert color="red">{error}</Alert>}
      <Paper withBorder p="lg">
        <Stack gap="xs">
          <Text><strong>Requested by:</strong> {request.requestedBy}</Text>
          <Text><strong>Created:</strong> {new Date(request.createdAt).toLocaleString()}</Text>
          <Text><strong>Description:</strong> {request.description}</Text>
          {request.rejectionReason && <Text c="red"><strong>Rejection reason:</strong> {request.rejectionReason}</Text>}
        </Stack>
      </Paper>
      {request.status === 'Pending' && (
        <Paper withBorder p="lg">
          <Stack>
            <Title order={4}>Review action</Title>
            <TextInput label="Changed by" required value={changedBy} onChange={(event) => setChangedBy(event.currentTarget.value)} />
            <Group>
              <Button color="green" loading={saving} disabled={!changedBy.trim()} onClick={approve}>Approve</Button>
              <Button color="red" variant="outline" onClick={() => setRejectOpened(true)}>Reject</Button>
            </Group>
          </Stack>
        </Paper>
      )}
      <Paper withBorder p="lg">
        <Title order={4}>Status history</Title>
        <Divider my="sm" />
        <List spacing="sm">
          {request.history.map((history) => (
            <List.Item key={history.id}>
              <Text size="sm">
                {history.fromStatus ?? 'New'} to {history.toStatus} by {history.changedBy} on {new Date(history.changedAt).toLocaleString()}
                {history.comment ? `: ${history.comment}` : ''}
              </Text>
            </List.Item>
          ))}
        </List>
      </Paper>
      <RejectRequestModal opened={rejectOpened} loading={saving} onClose={() => setRejectOpened(false)} onSubmit={reject} />
    </Stack>
  )
}
