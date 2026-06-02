import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import { localStorageColorSchemeManager, MantineProvider } from '@mantine/core'
import '@mantine/core/styles.css'
import './styles.css'
import App from './App'
import { AuthProvider } from './auth/AuthContext'
import { theme } from './theme'

const colorSchemeManager = localStorageColorSchemeManager({ key: 'reqflow-color-scheme' })

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <MantineProvider colorSchemeManager={colorSchemeManager} defaultColorScheme="light" theme={theme}>
      <AuthProvider>
        <BrowserRouter>
          <App />
        </BrowserRouter>
      </AuthProvider>
    </MantineProvider>
  </StrictMode>,
)
