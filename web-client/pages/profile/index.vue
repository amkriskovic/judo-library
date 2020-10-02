<template>
  <v-card>

    <!-- Basic Profile info -->
    <v-card-title>
      <v-avatar>
        <v-icon>mdi-account</v-icon>
      </v-avatar>
      Test User
    </v-card-title>

    <v-card-text>
      <div class="mx-2" v-if="submissions">
        <v-card class="mb-3" v-for="submission in submissions" :key="`${submission.id}`">
          <!-- Injecting video player component with dynamic binding of video where video is string -->
          <!-- * Getting specific videos from submissions store |> state => fetchSubmissionsForTechnique filling state -->
          <video-player :video="submission.video" :key="`v-${submission.id}`"/>

          <v-card-text>{{ submission.description }}</v-card-text>
        </v-card>
      </div>
    </v-card-text>

  </v-card>
</template>

<script>
import VideoPlayer from "@/components/video-player";

export default {
  comments: {VideoPlayer},

  // Local state
  data: () => ({
    // User should have some submissions
    submissions: []
  }),

  async mounted() {
    // Trigger _watchUserLoaded watcher which will try to synchronize, our store with client
    // * Payload -> action => kick off =>> this function/action should be executed only after loading is finished
    return this.$store.dispatch("auth/_watchUserLoaded", async () => {

      // Grabbing profile from auth.js state
      const profile = this.$store.state.auth.profile

      console.log('mounted profile:: ', profile)

      // Make HTTP GET req to our users controller which will return list of submissions based on profile id
      this.submissions = await this.$axios.$get(`/api/users/${profile.id}/submissions`)
    })
  }
}
</script>

<style scoped>

</style>
