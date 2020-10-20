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
      <div class="text-h5">
        <span>{{ technique.name }}</span>
        <!-- :to= corresponds to page that we created | category == folder | category.id == page?  -->
        <v-chip small label class="mb-1 ml-2" :to="`/category/${category.id}`">{{ category.name }}</v-chip>
        <v-chip small label class="mb-1 ml-2" :to="`/subcategory/${subcategory.id}`">{{ subcategory.name }}</v-chip>
      </div>

      <v-divider class="my-1"></v-divider>
      <div class="text-body-2"> {{ technique.description }}</div>
      <v-divider class="my-1"></v-divider>

      <!-- Auto-generated component based what relatedData func is returning -->
      <div v-for="rd in relatedData" v-if="rd.data.length > 0">
        <!-- * If there is data, only then display whole object inforation -->
        <div class="text-subtitle-1">{{ rd.title }}</div>

        <v-chip-group>
          <!-- If something is in the same category, component get's re-rendered (cached), d is data: which is technique(id) for now -->
          <v-chip v-for="d in rd.data" :key="rd.idFactory(d)" :to="rd.routeFactory(d)" small label>
            {{ d.name }}
          </v-chip>
        </v-chip-group>
      </div>

      <v-divider class="mb-2"></v-divider>

      <!-- Technique Edit Button -->
      <div>
        <!-- We run edit func first, then close func 2nd -->
        <v-btn outlined small @click="edit(); close()">Edit</v-btn>
      </div>
    </template>
  </item-content-layout>

</template>

<script>
import {mapState, mapMutations} from "vuex";
import ItemContentLayout from "../../components/item-content-layout";
import TechniqueSteps from "../../components/content-creation/techniques-steps"
import Submission from "@/components/submission";

export default {
  components: {Submission, ItemContentLayout},
  // Page local state
  data: () => ({
    technique: null,
    category: null,
    subcategory: null,
  }),

  // Map state for submissions and techniques, mapping getters for returning technique by id
  computed: {
    ...mapState("submissions", ["submissions"]),

    // Importing dictionary from initial state of techniques store => for particular technique we use dict => indexing
    ...mapState("techniques", ["dictionary"]),

    // Function that returns object with title, data -> for specific technique, related data in this case filters:
    // subcategory, setup attacks and followup attacks, idFactory for uniqueness and routeFactory for navigating
    // thorough chips to respective technique
    // * Already fetched
    relatedData() {
      return [
        {
          title: "Set Up Attacks",
          data: this.technique.setUpAttacks.map(t => this.dictionary.techniques[t]), // t is Id of technique
          idFactory: t => `technique-${t.id}`,
          routeFactory: t => `/technique/${t.slug}`,
        },
        {
          title: "Follow Up Attacks",
          data: this.technique.followUpAttacks.map(t => this.dictionary.techniques[t]), // t is Id of technique
          idFactory: t => `technique-${t.id}`,
          routeFactory: t => `/technique/${t.slug}`,
        },
        {
          title: "Counters",
          data: this.technique.counters.map(t => this.dictionary.techniques[t]), // t is Id of technique
          idFactory: t => `technique-${t.id}`,
          routeFactory: t => `/technique/${t.slug}`
        }
      ]
    },

  },

  methods: {
    ...mapMutations("video-upload", ["activate"]),

    // Method for editing Technique
    edit() {
      // Invoke activate mutation from video-upload store
      // Passing TechniqueSteps as component, set edit as true, and editPayload as this technique => page data/local state
      this.activate({component: TechniqueSteps, edit: true, editPayload: this.technique})
    }
  },

  // Pre-fetching data asynchronously for this particular page
  async fetch() {
    // Getting techniqueId from URL param
    const techniqueSlug = this.$route.params.technique

    // Assigning particular grabbed technique slug after /technique/... from url -> id to pages local state technique
    this.technique = this.dictionary.techniques[techniqueSlug]

    this.category = this.dictionary.categories[this.technique.category]

    this.subcategory = this.dictionary.subcategories[this.technique.subCategory]

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
