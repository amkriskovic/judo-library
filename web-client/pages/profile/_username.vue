<template>
  <item-content-layout>
    <template v-slot:content>
      <!-- Injecting submission-feed component -->
      <submission-feed v-if="profile" :content-endpoint="`/api/users/${profile.id}/submissions`"/>
    </template>

    <template v-slot:item>
      <!-- Basic Profile info -->
      <user-header v-if="profile" :username="profile.username" :image-url="profile.image" :link="false"/>
    </template>
  </item-content-layout>
</template>

<script>
import ItemContentLayout from "@/components/item-content-layout";
import Submission from "@/components/submission";
import SubmissionFeed from "@/components/submission-feed";
import UserHeader from "@/components/user-header";

export default {
  components: {UserHeader, SubmissionFeed, Submission, ItemContentLayout},

  // Local state
  data: () => ({
    profile: null
  }),

  async fetch() {
    // Getting username from url param
    const {username} = this.$route.params

    // Make API call to fetch user
    this.profile = await this.$axios.$get(`/api/users/${username}`)
  }

}
</script>

<style scoped>

</style>
