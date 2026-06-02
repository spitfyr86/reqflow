import { createContext, PropsWithChildren, useContext, useState } from 'react'
import { requestApprovalApi, setAccessToken } from '../api/requestApprovalApi'
import type { CurrentUser } from '../types/request'

interface AuthState {
  user?: CurrentUser
  login: (userId: string) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthState | undefined>(undefined)

export function AuthProvider({ children }: PropsWithChildren) {
  const [user, setUser] = useState<CurrentUser>()

  async function login(userId: string) {
    const response = await requestApprovalApi.login(userId)
    setAccessToken(response.accessToken)
    setUser(response.user)
  }

  function logout() {
    setAccessToken()
    setUser(undefined)
  }

  return <AuthContext.Provider value={{ user, login, logout }}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) throw new Error('useAuth must be used within AuthProvider')
  return context
}
