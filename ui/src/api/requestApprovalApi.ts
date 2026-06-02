import type { CreateRequest, RequestDetail, RequestListItem } from '../types/request'

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'

interface ProblemDetails {
  detail?: string
  title?: string
}

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    headers: { 'Content-Type': 'application/json', ...options?.headers },
    ...options,
  })

  if (!response.ok) {
    const problem = (await response.json().catch(() => ({}))) as ProblemDetails
    throw new Error(problem.detail ?? problem.title ?? `Request failed with status ${response.status}`)
  }

  return (await response.json()) as T
}

export const requestApprovalApi = {
  list: () => request<RequestListItem[]>('/api/requests'),
  get: (id: string) => request<RequestDetail>(`/api/requests/${id}`),
  create: (data: CreateRequest) =>
    request<RequestDetail>('/api/requests', { method: 'POST', body: JSON.stringify(data) }),
  approve: (id: string, changedBy: string) =>
    request<RequestDetail>(`/api/requests/${id}/approve`, { method: 'POST', body: JSON.stringify({ changedBy }) }),
  reject: (id: string, changedBy: string, reason: string) =>
    request<RequestDetail>(`/api/requests/${id}/reject`, { method: 'POST', body: JSON.stringify({ changedBy, reason }) }),
}
