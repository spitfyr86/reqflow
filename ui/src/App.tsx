import { AppShell, Avatar, Button, Container, Group, Stack, Text, Title } from '@mantine/core'
import { Route, Routes } from 'react-router-dom'
import { CreateRequestPage } from './pages/CreateRequestPage'
import { RequestDetailsPage } from './pages/RequestDetailsPage'
import { RequestsListPage } from './pages/RequestsListPage'
import { LoginPage } from './pages/LoginPage'
import { useAuth } from './auth/AuthContext'
import { ColorSchemeToggle } from './components/ColorSchemeToggle'
import { WorkflowBanner } from './components/WorkflowBanner'

export default function App() {
  const { user, logout } = useAuth()

  if (!user) return <LoginPage />

  return (
    <AppShell header={{ height: 72 }} padding={{ base: 'md', sm: 'xl' }}>
      <AppShell.Header className="app-header">
        <Container size="lg" h="100%">
          <Group h="100%" justify="space-between">
            <Group gap="sm">
              <div className="brand-mark">RF</div>
              <div>
                <Title order={3}>ReqFlow</Title>
                <Text c="dimmed" size="xs" fw={600}>REQUESTS + WORKFLOW</Text>
              </div>
            </Group>
            <Group gap="sm">
              <ColorSchemeToggle />
              <Group gap="xs" visibleFrom="sm">
                <Avatar color="navy" radius="xl">{user.displayName.slice(0, 2).toUpperCase()}</Avatar>
                <div>
                  <Text size="sm" fw={700}>{user.displayName}</Text>
                  <Text c="dimmed" size="xs">{user.role}</Text>
                </div>
              </Group>
              <Button variant="subtle" size="compact-sm" onClick={logout}>Sign out</Button>
            </Group>
          </Group>
        </Container>
      </AppShell.Header>
      <AppShell.Main>
        <Container size="lg">
          <Stack gap="xl">
            <WorkflowBanner />
            <Routes>
              <Route path="/" element={<RequestsListPage />} />
              <Route path="/requests/new" element={<CreateRequestPage />} />
              <Route path="/requests/:id" element={<RequestDetailsPage />} />
            </Routes>
          </Stack>
        </Container>
      </AppShell.Main>
    </AppShell>
  )
}
