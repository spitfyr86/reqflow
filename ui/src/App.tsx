import { AppShell, Container, Group, Text, Title } from '@mantine/core'
import { Route, Routes } from 'react-router-dom'
import { CreateRequestPage } from './pages/CreateRequestPage'
import { RequestDetailsPage } from './pages/RequestDetailsPage'
import { RequestsListPage } from './pages/RequestsListPage'

export default function App() {
  return (
    <AppShell header={{ height: 64 }} padding="md">
      <AppShell.Header>
        <Container size="lg" h="100%">
          <Group h="100%">
            <Title order={3}>ReqFlow</Title>
            <Text c="dimmed">Requests + Workflow</Text>
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
