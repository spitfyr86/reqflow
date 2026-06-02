import { Badge } from '@mantine/core'
import type { RequestStatus } from '../types/request'

const colors: Record<RequestStatus, string> = {
  Pending: 'yellow',
  Approved: 'green',
  Rejected: 'red',
}

export function RequestStatusBadge({ status }: { status: RequestStatus }) {
  return <Badge color={colors[status]}>{status}</Badge>
}
