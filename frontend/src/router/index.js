import { createRouter, createWebHistory } from "vue-router";
import UploadView from "../views/UploadView.vue";
import RenderingView from "../views/RenderingView.vue";

const routes = [
  {
    path: "/",
    name: "upload",
    component: UploadView
  },
  {
    path: "/rendering/:fileId",
    name: "rendering",
    component: RenderingView,
    props: true
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;
