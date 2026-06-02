import { AppShell, Avatar, Badge, Button, Container, Group, Indicator, Stack, Text, Title } from '@mantine/core'
import { Route, Routes, useLocation } from 'react-router-dom'
import { useEffect, useState } from 'react'
import { CreateRequestPage } from './pages/CreateRequestPage'
import { RequestDetailsPage } from './pages/RequestDetailsPage'
import { RequestsListPage } from './pages/RequestsListPage'
import { LoginPage } from './pages/LoginPage'
import { useAuth } from './auth/AuthContext'
import { ColorSchemeToggle } from './components/ColorSchemeToggle'
import { WorkflowBanner } from './components/WorkflowBanner'
import { requestApprovalApi } from './api/requestApprovalApi'

export default function App() {
  const { user, logout } = useAuth()
  const location = useLocation()
  const [pendingCount, setPendingCount] = useState(0)

  useEffect(() => {
    if (!user || !['Approver', 'Admin'].includes(user.role)) {
      setPendingCount(0)
      return
    }

    function refreshPendingCount() {
      requestApprovalApi.pendingCount()
        .then((response) => setPendingCount(response.count))
        .catch(() => setPendingCount(0))
    }

    refreshPendingCount()
    window.addEventListener('reqflow:requests-changed', refreshPendingCount)
    return () => window.removeEventListener('reqflow:requests-changed', refreshPendingCount)
  }, [location.pathname, user])

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
                <Indicator
                  color="red"
                  disabled={!['Approver', 'Admin'].includes(user.role) || pendingCount === 0}
                  inline
                  label={pendingCount}
                  size={18}
                >
                  <Avatar color="navy" radius="xl">{user.displayName.slice(0, 2).toUpperCase()}</Avatar>
                </Indicator>
                <div>
                  <Text size="sm" fw={700}>{user.displayName}</Text>
                  <Group gap={4}>
                    <Text c="dimmed" size="xs">{user.role}</Text>
                    {['Approver', 'Admin'].includes(user.role) && pendingCount > 0 && (
                      <Badge color="red" size="xs" variant="light">{pendingCount} pending</Badge>
                    )}
                  </Group>
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
