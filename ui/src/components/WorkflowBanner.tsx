import { Badge, Group, Paper, Stack, Text, Title } from '@mantine/core'

export function WorkflowBanner() {
  return (
    <Paper className="workflow-banner" p={{ base: 'lg', sm: 'xl' }}>
      <Stack gap="xs">
        <Badge color="white" c="navy.9" variant="filled" w="fit-content">Request approval workspace</Badge>
        <Title order={1} c="white">Move requests forward with clarity.</Title>
        <Text c="navy.0" maw={660}>
          Submit, review, and audit approval decisions in one focused workflow.
        </Text>
        <Group gap="xl" mt="sm">
          <Text size="sm" fw={600}>Role-aware actions</Text>
          <Text size="sm" fw={600}>Tracked decisions</Text>
          <Text size="sm" fw={600}>Reliable history</Text>
        </Group>
      </Stack>
    </Paper>
  )
}
