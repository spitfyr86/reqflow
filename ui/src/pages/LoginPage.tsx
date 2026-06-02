import { Alert, Button, Paper, Select, Stack, Text, Title } from '@mantine/core'
import { FormEvent, useEffect, useState } from 'react'
import { requestApprovalApi } from '../api/requestApprovalApi'
import { useAuth } from '../auth/AuthContext'
import type { CurrentUser } from '../types/request'

export function LoginPage() {
  const { login } = useAuth()
  const [users, setUsers] = useState<CurrentUser[]>([])
  const [userId, setUserId] = useState<string | null>(null)
  const [error, setError] = useState<string>()
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    requestApprovalApi.demoUsers()
      .then(setUsers)
      .catch((cause: Error) => setError(cause.message))
  }, [])

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!userId) return
    setLoading(true)
    setError(undefined)
    try {
      await login(userId)
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'Unable to sign in.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <Stack maw={520} mx="auto" mt="xl">
      <Title order={2}>Demo sign in</Title>
      <Text c="dimmed">Choose a seeded identity to exercise role-based request approval.</Text>
      {error && <Alert color="red">{error}</Alert>}
      <Paper component="form" withBorder p="lg" onSubmit={handleSubmit}>
        <Stack>
          <Select
            label="Demo user"
            placeholder="Select a user"
            data={users.map((user) => ({ value: user.id, label: `${user.displayName} (${user.role})` }))}
            value={userId}
            onChange={setUserId}
          />
          <Button type="submit" loading={loading} disabled={!userId}>Sign in</Button>
        </Stack>
      </Paper>
    </Stack>
  )
}
