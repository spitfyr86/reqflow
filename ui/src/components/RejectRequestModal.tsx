import { Button, Group, Modal, Textarea } from '@mantine/core'
import { useState } from 'react'

interface RejectRequestModalProps {
  opened: boolean
  loading: boolean
  onClose: () => void
  onSubmit: (reason: string) => Promise<void>
}

export function RejectRequestModal({ opened, loading, onClose, onSubmit }: RejectRequestModalProps) {
  const [reason, setReason] = useState('')

  return (
    <Modal opened={opened} onClose={onClose} title="Reject request">
      <Textarea label="Reason" required value={reason} onChange={(event) => setReason(event.currentTarget.value)} />
      <Group justify="flex-end" mt="lg">
        <Button variant="default" onClick={onClose}>Cancel</Button>
        <Button color="red" loading={loading} disabled={!reason.trim()} onClick={() => onSubmit(reason)}>
          Reject
        </Button>
      </Group>
    </Modal>
  )
}
