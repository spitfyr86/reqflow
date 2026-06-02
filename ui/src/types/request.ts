export type RequestStatus = 'Pending' | 'Approved' | 'Rejected'

export interface RequestListItem {
  id: string
  title: string
  requestedBy: string
  status: RequestStatus
  createdAt: string
}

export interface RequestHistory {
  id: string
  fromStatus: RequestStatus | null
  toStatus: RequestStatus
  changedBy: string
  changedAt: string
  comment: string | null
}

export interface RequestDetail extends RequestListItem {
  description: string
  requestedByUserId: string
  updatedAt: string | null
  approvedRejectedBy: string | null
  approvedRejectedByUserId: string | null
  approvedRejectedAt: string | null
  rejectionReason: string | null
  history: RequestHistory[]
}

export interface CreateRequest {
  title: string
  description: string
}

export type UserRole = 'Requester' | 'Approver' | 'Admin'

export interface CurrentUser {
  id: string
  email: string
  displayName: string
  role: UserRole
}

export interface LoginResponse {
  accessToken: string
  user: CurrentUser
}
