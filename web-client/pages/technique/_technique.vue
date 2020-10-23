<template>

  <!-- Item-Content component injection -->
  <item-content-layout>
    <!-- Template for Content-->
    <template v-slot:content>
      <!-- Injecting submission component -> passing submission as prop from looped collection of submissions -->
      <submission :submission="submission" v-for="submission in submissions" :key="`submission-${submission.id}`"/>
    </template>

    <!-- Template for Item(card) -->
    <!-- Item template == right, expand the close func -->
    <template v-slot:item="{close}">

      <!-- Injecting technique-info-card component -->
      <technique-info-card :technique="technique" :close="close"/>

    </template>
  </item-content-layout>

</template>

<script>
import {mapState, mapMutations} from "vuex";
import ItemContentLayout from "../../components/item-content-layout";
import TechniqueSteps from "../../components/content-creation/techniques-steps"
import Submission from "@/components/submission";
import TechniqueInfoCard from "@/components/technique-info-card";

export default {
  components: {TechniqueInfoCard, Submission, ItemContentLayout},
  // Page local state
  data: () => ({
    technique: null,
  }),

  // Map state for submissions and techniques, mapping getters for returning technique by id
  computed: {
    ...mapState("submissions", ["submissions"]),

    // Importing dictionary from initial state of techniques store => for particular technique we use dict => indexing
    ...mapState("techniques", ["dictionary"]),
  },

  // Pre-fetching data asynchronously for this particular page
  async fetch() {
    // Getting techniqueId from URL param
    const techniqueSlug = this.$route.params.technique

    // Assigning particular grabbed technique slug after /technique/... from url -> id to pages local state technique
    this.technique = this.dictionary.techniques[techniqueSlug]

    // dispatching fetchSubmissionsForTechnique action from submissions store, passing techniqueId as argument
    await this.$store.dispatch("submissions/fetchSubmissionsForTechnique", {techniqueId: techniqueSlug}, {root: true}); // Dispatch action as root
  },

  // Setting via head method the HTML Head tags for the current page.
  head() {
    // If there is no technique return empty object
    if (!this.technique) return {}

    return {
      title: this.technique.name,
      meta: [
        {hid: 'description', name: 'description', content: this.technique.description}
      ]
    }
  }

}
</script>

<style scoped>

</style>
