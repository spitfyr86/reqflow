import { Anchor, ScrollArea, Table, Text } from '@mantine/core'
import { Link } from 'react-router-dom'
import type { RequestListItem } from '../types/request'
import { RequestStatusBadge } from './RequestStatusBadge'

export function RequestTable({ requests }: { requests: RequestListItem[] }) {
  const rows = requests.map((request) => (
    <Table.Tr key={request.id}>
      <Table.Td><Text fw={600}>{request.title}</Text></Table.Td>
      <Table.Td>{request.requestedBy}</Table.Td>
      <Table.Td><RequestStatusBadge status={request.status} /></Table.Td>
      <Table.Td>{new Date(request.createdAt).toLocaleString()}</Table.Td>
      <Table.Td><Anchor component={Link} fw={600} to={`/requests/${request.id}`}>Open</Anchor></Table.Td>
    </Table.Tr>
  ))

  return (
    <ScrollArea>
      <Table highlightOnHover verticalSpacing="md">
        <Table.Thead>
          <Table.Tr>
            <Table.Th>Request</Table.Th>
            <Table.Th>Requested by</Table.Th>
            <Table.Th>Status</Table.Th>
            <Table.Th>Created at</Table.Th>
            <Table.Th>Actions</Table.Th>
          </Table.Tr>
        </Table.Thead>
        <Table.Tbody>{rows}</Table.Tbody>
      </Table>
    </ScrollArea>
  )
}
