<template>
  <item-content-layout>
    <template v-slot:content>
      <div class="mx-2" v-if="submissions">
        <v-card class="mb-3" v-for="submission in submissions" :key="`${submission.id}`">
          <!-- Injecting video player component with dynamic binding of video where video is string -->
          <!-- * Getting specific videos from submissions store |> state => fetchSubmissionsForTechnique filling state -->
          <video-player :video="submission.video" :key="`v-${submission.id}`"/>

          <v-card-text>{{ submission.description }}</v-card-text>
        </v-card>
      </div>
    </template>

    <template v-slot:item>
      <!-- Basic Profile info -->
      <div v-if="profile">
        <div>
          <!-- Input -->
          <input class="d-none" type="file" accept="image/*" ref="profileImageInput" @change="changeProfileImage"/>
          <v-hover v-slot:default="{hover}">
            <v-avatar>
              <!-- Proxies click event when we hover over profile icon -> accepting image file -->
              <!-- Disabled if uploadingImage is true | false initial value -->
              <v-btn v-if="hover" icon :disabled="uploadingImage" @click="$refs.profileImageInput.click()">
                <v-icon>mdi-account-edit</v-icon>
              </v-btn>

              <!-- Make use of videos controller method for getting the profile image-->
              <img v-else-if="profile.image" :src="profile.image"
                   alt="profile image"/>

              <v-icon v-else>mdi-account</v-icon>
            </v-avatar>
          </v-hover>
          {{ profile.username }}
        </div>
      </div>
    </template>
  </item-content-layout>
</template>

<script>
import VideoPlayer from "@/components/video-player";
import ItemContentLayout from "@/components/item-content-layout";
import {mapMutations, mapState} from "vuex";

export default {
  components: {ItemContentLayout, VideoPlayer},

  // Local state
  data: () => ({
    // User should have some submissions
    submissions: [],

    uploadingImage: false
  }),

  // Called when DOM is rendered and reactive
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
  },

  methods: {
    // On changing profile image
    changeProfileImage(e) {
      // If uploadingImage is true (false initial value), return
      if (this.uploadingImage) return

      // Set uploadingImage to true, to kick off
      this.uploadingImage = true

      // Extract file input from change event
      const fileInput = e.target

      // Extract the file (image name)
      const file = fileInput.files[0]

      // Create Form for POST
      const formData = new FormData();

      // Append to form file -> image => key:value list => needs to be called Image in backend models
      formData.append('image', file)

      // PUT => always update => put the image to our (Users Controller) profile image endpoint, send formData as payload
      return this.$axios.$put(`/api/users/me/image`, formData)
        .then(profile => {

          // Passing profile to saveProfile from auth.js -> mutations
          this.saveProfile({profile})

          // Reset the input value
          fileInput.value = ""

          // Set uploading image to false -> finished
          this.uploadingImage = false
        })
    },

    ...mapMutations("auth", ["saveProfile"])
  },

  computed: mapState("auth", ["profile"])
}
</script>

<style scoped>

</style>
