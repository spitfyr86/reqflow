import { Alert } from '@mantine/core'

export function ErrorAlert({ message }: { message: string }) {
  return <Alert color="red" title="Something went wrong">{message}</Alert>
}
