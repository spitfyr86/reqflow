import type { CreateRequest, CurrentUser, LoginResponse, PendingRequestCount, RequestDetail, RequestListItem } from '../types/request'

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'

interface ProblemDetails {
  detail?: string
  title?: string
}

let accessToken: string | undefined

export function setAccessToken(token?: string) {
  accessToken = token
}

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
      ...options?.headers,
    },
    ...options,
  })

  if (!response.ok) {
    const problem = (await response.json().catch(() => ({}))) as ProblemDetails
    throw new Error(problem.detail ?? problem.title ?? `Request failed with status ${response.status}`)
  }

  return (await response.json()) as T
}

export const requestApprovalApi = {
  demoUsers: () => request<CurrentUser[]>('/api/auth/demo-users'),
  login: (userId: string) =>
    request<LoginResponse>('/api/auth/demo-login', { method: 'POST', body: JSON.stringify({ userId }) }),
  list: () => request<RequestListItem[]>('/api/requests'),
  pendingCount: () => request<PendingRequestCount>('/api/requests/pending-count'),
  get: (id: string) => request<RequestDetail>(`/api/requests/${id}`),
  create: (data: CreateRequest) =>
    request<RequestDetail>('/api/requests', { method: 'POST', body: JSON.stringify(data) }),
  approve: (id: string) =>
    request<RequestDetail>(`/api/requests/${id}/approve`, { method: 'POST' }),
  reject: (id: string, reason: string) =>
    request<RequestDetail>(`/api/requests/${id}/reject`, { method: 'POST', body: JSON.stringify({ reason }) }),
}
