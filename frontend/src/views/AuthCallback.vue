<script setup lang="ts">
import { onMounted } from 'vue';
import { useRouter } from 'vue-router';
import userManager from '../services/auth';

const router = useRouter();

onMounted(async () => {
  try {
    await userManager.signinCallback();
  } catch (err) {
    console.error('Erro no callback', err);
  } finally {
    router.push('/');
  }
});
</script>

<template>
  <p>Processando login...</p>
</template>  