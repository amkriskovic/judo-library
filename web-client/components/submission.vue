<template>
  <v-card class="mb-3">
    <!-- Injecting user-header component, providing username from submission -->
    <user-header :username="submission.user.username" :image-url="submission.user.image"/>

    <!-- Injecting video player component with dynamic binding of video where video is string -->
    <!-- * Getting specific videos from submissions store |> state => fetchSubmissionsForTechnique filling state -->
    <video-player :video="submission.video" :thumb="submission.thumb"/>

    <v-card-text>{{ submission.description }}</v-card-text>
  </v-card>
</template>

<script>
import UserHeader from "@/components/user-header";
import VideoPlayer from "@/components/video-player";
export default {
  name: "submission",
  components: {VideoPlayer, UserHeader},

  // Defining that submission needs to be passed as an object
  props: {
    submission: {
      type: Object,
      required: true,
    }
  }
}
</script>

<style scoped>

</style>
