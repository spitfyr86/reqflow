import { Button, Group, Modal, Textarea, TextInput } from '@mantine/core'
import { useState } from 'react'

interface RejectRequestModalProps {
  opened: boolean
  loading: boolean
  onClose: () => void
  onSubmit: (changedBy: string, reason: string) => Promise<void>
}

export function RejectRequestModal({ opened, loading, onClose, onSubmit }: RejectRequestModalProps) {
  const [changedBy, setChangedBy] = useState('')
  const [reason, setReason] = useState('')

  return (
    <Modal opened={opened} onClose={onClose} title="Reject request">
      <TextInput label="Changed by" required value={changedBy} onChange={(event) => setChangedBy(event.currentTarget.value)} />
      <Textarea mt="md" label="Reason" required value={reason} onChange={(event) => setReason(event.currentTarget.value)} />
      <Group justify="flex-end" mt="lg">
        <Button variant="default" onClick={onClose}>Cancel</Button>
        <Button color="red" loading={loading} disabled={!changedBy.trim() || !reason.trim()} onClick={() => onSubmit(changedBy, reason)}>
          Reject
        </Button>
      </Group>
    </Modal>
  )
}
