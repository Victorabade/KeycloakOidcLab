import { createRouter, createWebHistory } from 'vue-router';
import AuthCallbackView from '../views/AuthCallback.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: () => import('../views/HomeView.vue'),
    },
    {
      path: '/auth-callback',
      component: AuthCallbackView,
    },
  ],
});

export default router;