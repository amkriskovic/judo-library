<template>
  <!-- Item-Content component injection -->
  <item-content-layout>
    <!-- Template for Content-->
    <template v-slot:content>
      <!-- Injecting submission feed component which will take care for loading content feed -->
      <submission-feed :content-endpoint="`/api/techniques/${technique.slug}/submissions`"/>
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
import {mapState} from "vuex";
import ItemContentLayout from "@/components/item-content-layout";
import Submission from "@/components/submission";
import TechniqueInfoCard from "@/components/technique-info-card";
import SubmissionFeed from "@/components/submission-feed";

export default {
  components: {SubmissionFeed, TechniqueInfoCard, Submission, ItemContentLayout},

  // Map state for submissions and techniques, mapping getters for returning technique by id
  computed: {
    // Importing dictionary from initial state of techniques store => for particular technique we use dict => indexing
    ...mapState("library", ["dictionary"]),

    technique() {
      // Getting techniqueId from URL param
      // Assigning particular grabbed technique slug after /technique/... from url -> id to pages local state technique
      return this.dictionary.techniques[this.$route.params.technique]
    }
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
