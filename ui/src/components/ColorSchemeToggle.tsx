import { ActionIcon, Tooltip, useMantineColorScheme } from '@mantine/core'

function MoonIcon() {
  return (
    <svg aria-hidden="true" fill="none" height="18" stroke="currentColor" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" viewBox="0 0 24 24" width="18">
      <path d="M21 12.8A9 9 0 1 1 11.2 3 7 7 0 0 0 21 12.8Z" />
    </svg>
  )
}

function SunIcon() {
  return (
    <svg aria-hidden="true" fill="none" height="18" stroke="currentColor" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" viewBox="0 0 24 24" width="18">
      <circle cx="12" cy="12" r="4" />
      <path d="M12 2v2M12 20v2M4.9 4.9l1.4 1.4M17.7 17.7l1.4 1.4M2 12h2M20 12h2M4.9 19.1l1.4-1.4M17.7 6.3l1.4-1.4" />
    </svg>
  )
}

export function ColorSchemeToggle() {
  const { colorScheme, setColorScheme } = useMantineColorScheme()
  const dark = colorScheme === 'dark'

  return (
    <Tooltip label={`Use ${dark ? 'light' : 'dark'} mode`}>
      <ActionIcon
        aria-label={`Use ${dark ? 'light' : 'dark'} mode`}
        color="navy"
        onClick={() => setColorScheme(dark ? 'light' : 'dark')}
        size="lg"
        variant="light"
      >
        {dark ? <SunIcon /> : <MoonIcon />}
      </ActionIcon>
    </Tooltip>
  )
}
