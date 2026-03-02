import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    host: '0.0.0.0',
    port: 3000,
    proxy: {
      '/auth': 'http://api-gateway:8080',
      '/products': 'http://api-gateway:8080',
      '/orders': 'http://api-gateway:8080'
    }
  }
})
