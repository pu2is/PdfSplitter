import { createRouter, createWebHistory, type RouteRecordRaw } from "vue-router";
import ChooseFile from "../views/ChooseFile.vue";

const routes: RouteRecordRaw[] = [
  {
    path: "/",
    name: "upload",
    component: ChooseFile
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;
