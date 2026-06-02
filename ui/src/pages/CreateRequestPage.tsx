import { Alert, Button, Group, Paper, Stack, Text, Textarea, TextInput, Title } from '@mantine/core'
import { FormEvent, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { requestApprovalApi } from '../api/requestApprovalApi'

export function CreateRequestPage() {
  const navigate = useNavigate()
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [error, setError] = useState<string>()
  const [loading, setLoading] = useState(false)

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setLoading(true)
    setError(undefined)
    try {
      const created = await requestApprovalApi.create({ title, description })
      navigate(`/requests/${created.id}`)
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'Unable to create request.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <Stack maw={760}>
      <div>
        <Title order={2}>Create request</Title>
        <Text c="dimmed">Describe the request clearly so reviewers can make a confident decision.</Text>
      </div>
      {error && <Alert color="red" title="Unable to create request">{error}</Alert>}
      <Paper className="content-card" component="form" withBorder p="xl" onSubmit={handleSubmit}>
        <Stack>
          <TextInput label="Title" required maxLength={150} value={title} onChange={(event) => setTitle(event.currentTarget.value)} />
          <Textarea label="Description" required maxLength={1000} minRows={4} value={description} onChange={(event) => setDescription(event.currentTarget.value)} />
          <Group justify="flex-end">
            <Button variant="default" component={Link} to="/">Cancel</Button>
            <Button type="submit" loading={loading}>Create request</Button>
          </Group>
        </Stack>
      </Paper>
    </Stack>
  )
}
