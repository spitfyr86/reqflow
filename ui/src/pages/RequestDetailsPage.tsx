import { Alert, Button, Divider, Grid, Group, List, Paper, Stack, Text, Title } from '@mantine/core'
import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { requestApprovalApi } from '../api/requestApprovalApi'
import { ErrorAlert } from '../components/ErrorAlert'
import { LoadingView } from '../components/LoadingView'
import { RejectRequestModal } from '../components/RejectRequestModal'
import { RequestStatusBadge } from '../components/RequestStatusBadge'
import type { RequestDetail } from '../types/request'
import { useAuth } from '../auth/AuthContext'

export function RequestDetailsPage() {
  const { id = '' } = useParams()
  const { user } = useAuth()
  const [request, setRequest] = useState<RequestDetail>()
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
      setRequest(await requestApprovalApi.approve(id))
      window.dispatchEvent(new Event('reqflow:requests-changed'))
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'Unable to approve request.')
    } finally {
      setSaving(false)
    }
  }

  async function reject(reason: string) {
    setSaving(true)
    setError(undefined)
    try {
      setRequest(await requestApprovalApi.reject(id, reason))
      window.dispatchEvent(new Event('reqflow:requests-changed'))
      setRejectOpened(false)
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'Unable to reject request.')
    } finally {
      setSaving(false)
    }
  }

  if (loading) return <LoadingView />
  if (!request) return <ErrorAlert message={error ?? 'Request was not found.'} />
  const canReview = user && ['Approver', 'Admin'].includes(user.role) && user.id !== request.requestedByUserId
  const reviewMessage = user?.id === request.requestedByUserId
    ? 'You created this request. To preserve separation of duties, another approver or administrator must review it.'
    : 'Your current role can view this request but cannot approve or reject it.'

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
      <Grid>
        <Grid.Col span={{ base: 12, md: 8 }}>
          <Paper className="content-card" withBorder p="xl" h="100%">
            <Stack gap="xs">
              <Text c="dimmed" size="sm" fw={700}>REQUEST DESCRIPTION</Text>
              <Text>{request.description}</Text>
              {request.rejectionReason && <Alert color="red" mt="md" title="Rejection reason">{request.rejectionReason}</Alert>}
            </Stack>
          </Paper>
        </Grid.Col>
        <Grid.Col span={{ base: 12, md: 4 }}>
          <Paper className="content-card" withBorder p="xl" h="100%">
            <Stack gap="xs">
              <Text c="dimmed" size="sm" fw={700}>REQUEST DETAILS</Text>
              <Text size="sm"><strong>Requested by</strong><br />{request.requestedBy}</Text>
              <Text size="sm"><strong>Created</strong><br />{new Date(request.createdAt).toLocaleString()}</Text>
              {request.approvedRejectedBy && <Text size="sm"><strong>Reviewed by</strong><br />{request.approvedRejectedBy}</Text>}
            </Stack>
          </Paper>
        </Grid.Col>
      </Grid>
      {request.status === 'Pending' && !canReview && (
        <Alert color="blue" title="Review action unavailable">{reviewMessage}</Alert>
      )}
      {request.status === 'Pending' && canReview && (
        <Paper className="content-card" withBorder p="lg">
          <Stack>
            <Title order={4}>Review action</Title>
            <Group>
              <Button color="green" loading={saving} onClick={approve}>Approve</Button>
              <Button color="red" variant="outline" onClick={() => setRejectOpened(true)}>Reject</Button>
            </Group>
          </Stack>
        </Paper>
      )}
      <Paper className="content-card" withBorder p="lg">
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
