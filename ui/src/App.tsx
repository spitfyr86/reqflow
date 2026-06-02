import { AppShell, Button, Container, Group, Text, Title } from '@mantine/core'
import { Route, Routes } from 'react-router-dom'
import { CreateRequestPage } from './pages/CreateRequestPage'
import { RequestDetailsPage } from './pages/RequestDetailsPage'
import { RequestsListPage } from './pages/RequestsListPage'
import { LoginPage } from './pages/LoginPage'
import { useAuth } from './auth/AuthContext'

export default function App() {
  const { user, logout } = useAuth()

  if (!user) return <LoginPage />

  return (
    <AppShell header={{ height: 64 }} padding="md">
      <AppShell.Header>
        <Container size="lg" h="100%">
          <Group h="100%" justify="space-between">
            <Group>
              <Title order={3}>ReqFlow</Title>
              <Text c="dimmed">Requests + Workflow</Text>
            </Group>
            <Group>
              <Text size="sm">{user.displayName} ({user.role})</Text>
              <Button variant="subtle" size="compact-sm" onClick={logout}>Sign out</Button>
            </Group>
          </Group>
        </Container>
      </AppShell.Header>
      <AppShell.Main>
        <Container size="lg">
          <Routes>
            <Route path="/" element={<RequestsListPage />} />
            <Route path="/requests/new" element={<CreateRequestPage />} />
            <Route path="/requests/:id" element={<RequestDetailsPage />} />
          </Routes>
        </Container>
      </AppShell.Main>
    </AppShell>
  )
}
