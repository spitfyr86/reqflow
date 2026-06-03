import { Alert, Badge, Box, Button, Container, Group, Paper, Select, Stack, Text, Title } from '@mantine/core'
import { FormEvent, useEffect, useState } from 'react'
import { requestApprovalApi } from '../api/requestApprovalApi'
import { useAuth } from '../auth/AuthContext'
import type { CurrentUser } from '../types/request'
import { ColorSchemeToggle } from '../components/ColorSchemeToggle'

export function LoginPage() {
  const { login } = useAuth()
  const [users, setUsers] = useState<CurrentUser[]>([])
  const [userId, setUserId] = useState<string | null>(null)
  const [error, setError] = useState<string>()
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    requestApprovalApi.users()
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
    <Box className="login-shell">
      <Container size="lg" py="xl">
        <Group justify="space-between">
          <Group gap="sm">
            <div className="brand-mark">RF</div>
            <div>
              <Title order={3}>ReqFlow</Title>
              <Text c="dimmed" size="xs" fw={600}>REQUESTS + WORKFLOW</Text>
            </div>
          </Group>
          <ColorSchemeToggle />
        </Group>
        <Paper className="content-card" maw={560} mx="auto" mt={80} p={{ base: 'lg', sm: 'xl' }} withBorder>
          <Stack gap="lg">
            <div>
              <Badge color="navy" variant="light" mb="sm">Demo workspace</Badge>
              <Title order={1}>Welcome to ReqFlow</Title>
              <Text c="dimmed" mt="xs">Choose a seeded identity to explore a secure, role-aware approval workflow.</Text>
            </div>
            {error && <Alert color="red">{error}</Alert>}
            <Paper component="form" bg="var(--mantine-color-default-hover)" p="lg" onSubmit={handleSubmit}>
              <Stack>
                <Select
                  label="Demo user"
                  description="Each identity demonstrates a different permission level."
                  placeholder="Select a user"
                  data={users.map((user) => ({ value: user.id, label: `${user.displayName} (${user.role})` }))}
                  value={userId}
                  onChange={setUserId}
                />
                <Button type="submit" loading={loading} disabled={!userId}>Enter workspace</Button>
              </Stack>
            </Paper>
            <Text c="dimmed" size="xs">Local demo authentication only. Production deployments should use an OIDC identity provider.</Text>
          </Stack>
        </Paper>
      </Container>
    </Box>
  )
}
