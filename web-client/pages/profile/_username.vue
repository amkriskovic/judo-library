<template>
  <item-content-layout v-if="profile">
    <template v-slot:content>
      <!-- Injecting submission-feed component -->
      <submission-feed :content-endpoint="`/api/users/${profile.id}/submissions`"/>
    </template>

    <template v-slot:item>
      <!-- Basic Profile info -->
      <user-header :username="profile.username" :image-url="profile.image" :link="false"/>

      <v-divider class="my-2"/>

      <div>
        <h6 class="text-h6 mb-2">Completed Techniques ({{completedTechniques.length}} / {{lists.techniques.length}})</h6>
        <v-chip class="mb-1 mr-1" small
                v-for="{submission, technique} in completedTechniques"
                @click="gotoSubmission(technique.slug, submission.id)"
                :key="`profile-technique-chip-${submission.id}`">
          {{ technique.name }}
        </v-chip>
      </div>

    </template>
  </item-content-layout>
</template>

<script>
import ItemContentLayout from "@/components/item-content-layout";
import Submission from "@/components/submission";
import SubmissionFeed from "@/components/submission-feed";
import UserHeader from "@/components/user-header";
import profile from "@/components/profile";

export default {
  components: {UserHeader, SubmissionFeed, Submission, ItemContentLayout},

  mixins: [profile],

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
