import { Anchor, Table } from '@mantine/core'
import { Link } from 'react-router-dom'
import type { RequestListItem } from '../types/request'
import { RequestStatusBadge } from './RequestStatusBadge'

export function RequestTable({ requests }: { requests: RequestListItem[] }) {
  const rows = requests.map((request) => (
    <Table.Tr key={request.id}>
      <Table.Td>{request.title}</Table.Td>
      <Table.Td>{request.requestedBy}</Table.Td>
      <Table.Td><RequestStatusBadge status={request.status} /></Table.Td>
      <Table.Td>{new Date(request.createdAt).toLocaleString()}</Table.Td>
      <Table.Td><Anchor component={Link} to={`/requests/${request.id}`}>View</Anchor></Table.Td>
    </Table.Tr>
  ))

  return (
    <Table striped highlightOnHover>
      <Table.Thead>
        <Table.Tr>
          <Table.Th>Title</Table.Th>
          <Table.Th>Requested by</Table.Th>
          <Table.Th>Status</Table.Th>
          <Table.Th>Created at</Table.Th>
          <Table.Th>Actions</Table.Th>
        </Table.Tr>
      </Table.Thead>
      <Table.Tbody>{rows}</Table.Tbody>
    </Table>
  )
}
