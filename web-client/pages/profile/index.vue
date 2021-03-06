﻿<template>
  <item-content-layout>
    <template v-slot:content>
      <!-- Injecting submission-feed component -->
      <submission-feed :content-endpoint="`/api/users/${$store.state.auth.profile.id}/submissions`"/>
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
              <img v-else-if="profile.image" :src="profile.image" alt="profile image"/>

              <v-icon v-else>mdi-account</v-icon>
            </v-avatar>
          </v-hover>
          {{ profile.username }}
        </div>

        <v-divider class="my-2"/>

        <profile-completed-techniques :profile-submissions="profile.submissions"/>

        <v-divider class="my-2"/>
        <h5 class="text-h5">Change Requests</h5>
        <v-list>
          <v-list-item v-for="modItem in moderationItems"
                       :key="`profile-modItem-${modItem.id}`"
                       :to="`/moderation/${modItem.id}`">
            <v-list-item-content>
              <v-list-item-title v-if="modItem.currentObject">{{ modItem.currentObject.name }}</v-list-item-title>
              <v-list-item-title v-else-if="modItem.targetObject">{{ modItem.targetObject.name }}</v-list-item-title>
              <v-list-item-subtitle>
                <span>Type: {{ modItem.type }},</span>
                <span v-if="modItem.currentObject">Version: {{ modItem.currentObject.version }}</span>
                <span v-else-if="modItem.targetObject">Version: {{ modItem.targetObject.version }}</span>
              </v-list-item-subtitle>
            </v-list-item-content>
          </v-list-item>
        </v-list>

      </div>
    </template>
  </item-content-layout>
</template>

<script>
import ItemContentLayout from "@/components/item-content-layout";
import {mapMutations, mapState} from "vuex";
import Submission from "@/components/submission";
import SubmissionFeed from "@/components/submission-feed";
import ProfileCompletedTechniques from "@/components/profile-completed-techniques";

export default {
  components: {ProfileCompletedTechniques, SubmissionFeed, Submission, ItemContentLayout},

  // Local state
  data: () => ({
    uploadingImage: false,
    moderationItems: []
  }),

  fetch() {
    return this.$axios.$get(`/api/moderation-items?user=1`)
      .then(moderationItems => this.moderationItems = moderationItems)
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

    ...mapMutations("auth", ["saveProfile"]),
  },

  computed: mapState("auth", ["profile"]),

}
</script>

<style scoped>

</style>
