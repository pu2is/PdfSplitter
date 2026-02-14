import { createRouter, createWebHistory, type RouteRecordRaw } from "vue-router";
import UploadView from "../views/UploadView.vue";

const routes: RouteRecordRaw[] = [
  {
    path: "/",
    name: "upload",
    component: UploadView
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;
