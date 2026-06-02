import { createTheme, MantineColorsTuple } from '@mantine/core'

const navy: MantineColorsTuple = [
  '#edf4ff',
  '#d9e7fb',
  '#adccef',
  '#7eafe4',
  '#5797da',
  '#4087d5',
  '#327fd3',
  '#246dbb',
  '#185fa9',
  '#064f97',
]

export const theme = createTheme({
  primaryColor: 'navy',
  colors: { navy },
  defaultRadius: 'md',
  fontFamily: 'Inter, Segoe UI, sans-serif',
  headings: { fontFamily: 'Inter, Segoe UI, sans-serif', fontWeight: '700' },
  components: {
    Button: { defaultProps: { radius: 'md' } },
    Paper: { defaultProps: { radius: 'lg' } },
  },
})
