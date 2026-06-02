import { Button, Grid, Group, Paper, Stack, Text, Title } from '@mantine/core'
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
  const countByStatus = (status: RequestListItem['status']) => requests.filter((request) => request.status === status).length

  return (
    <Stack>
      <Group align="end" justify="space-between">
        <div>
          <Title order={2}>Requests</Title>
          <Text c="dimmed">Create and review approval requests.</Text>
        </div>
        <Button component={Link} size="md" to="/requests/new">Create request</Button>
      </Group>
      <Grid>
        <Grid.Col span={{ base: 12, sm: 4 }}>
          <Paper className="summary-card" p="lg" withBorder>
            <Text c="dimmed" size="sm" fw={600}>Pending review</Text>
            <Title order={2}>{countByStatus('Pending')}</Title>
          </Paper>
        </Grid.Col>
        <Grid.Col span={{ base: 12, sm: 4 }}>
          <Paper className="summary-card" p="lg" withBorder>
            <Text c="dimmed" size="sm" fw={600}>Approved</Text>
            <Title order={2}>{countByStatus('Approved')}</Title>
          </Paper>
        </Grid.Col>
        <Grid.Col span={{ base: 12, sm: 4 }}>
          <Paper className="summary-card" p="lg" withBorder>
            <Text c="dimmed" size="sm" fw={600}>Rejected</Text>
            <Title order={2}>{countByStatus('Rejected')}</Title>
          </Paper>
        </Grid.Col>
      </Grid>
      {error && <ErrorAlert message={error} />}
      {loading ? <LoadingView /> : (
        <Paper className="content-card" withBorder p="md">
          {requests.length ? <RequestTable requests={requests} /> : <Text c="dimmed">No requests yet.</Text>}
        </Paper>
      )}
    </Stack>
  )
}
