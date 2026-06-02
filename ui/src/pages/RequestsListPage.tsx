import { Button, Group, Paper, Stack, Text, Title } from '@mantine/core'
import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { requestApprovalApi } from '../api/requestApprovalApi'
import { ErrorAlert } from '../components/ErrorAlert'
import { LoadingView } from '../components/LoadingView'
import { RequestTable } from '../components/RequestTable'
import type { RequestListItem } from '../types/request'

export function RequestsListPage() {
  const [requests, setRequests] = useState<RequestListItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string>()

  useEffect(() => {
    requestApprovalApi.list()
      .then(setRequests)
      .catch((cause: Error) => setError(cause.message))
      .finally(() => setLoading(false))
  }, [])

  return (
    <Stack>
      <Group justify="space-between">
        <div>
          <Title order={2}>Requests</Title>
          <Text c="dimmed">Create and review approval requests.</Text>
        </div>
        <Button component={Link} to="/requests/new">Create request</Button>
      </Group>
      {error && <ErrorAlert message={error} />}
      {loading ? <LoadingView /> : (
        <Paper withBorder p="md">
          {requests.length ? <RequestTable requests={requests} /> : <Text c="dimmed">No requests yet.</Text>}
        </Paper>
      )}
    </Stack>
  )
}
