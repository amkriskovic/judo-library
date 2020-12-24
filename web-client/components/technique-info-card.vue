<template>
  <v-sheet class="pa-3" rounded min-width="300">
    <div class="text-h5">
      <nuxt-link class="white--text text-decoration-none"
                 :to="`/technique/${technique.slug}`"
                 v-if="link">{{technique.name}}
      </nuxt-link>

      <span v-else>{{ technique.name }}</span>
      <!-- :to= corresponds to page that we created | category == folder | category.id == page?  -->
      <v-chip small label class="mb-1 ml-2" :to="`/category/${category.slug}`">{{ category.name }}</v-chip>
      <v-chip small label class="mb-1 ml-2" :to="`/subcategory/${subcategory.slug}`">{{ subcategory.name }}</v-chip>
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

    <if-auth>
      <!-- If we are authenticated => allowed -->
      <template v-slot:allowed>
        <div>
          <!-- We run edit func first, then close func 2nd -->
          <v-btn outlined small @click="edit(); close()">Edit</v-btn>
          <v-btn outlined small @click="upload(); close()">Upload</v-btn>
        </div>
      </template>

      <!-- If we are not authenticated => forbidden -->
      <template v-slot:forbidden="{login}">
        <div class="d-flex justify-center">
          <v-btn small outlined @click="login">
            Log in to edit/update
          </v-btn>
        </div>
      </template>
    </if-auth>

    <v-divider class="mt-2"></v-divider>

    <!-- Injecting user-header component for displaying User info -->
    <user-header class="pa-2" :username="technique.user.username" :image-url="technique.user.image" reverse>
      <template v-slot:append>
        <span>{{ technique.version === 1 ? `Created by` : `Edited by` }}</span>
      </template>
    </user-header>
  </v-sheet>

</template>

<script>
import {mapMutations, mapState} from "vuex";
import TechniqueSteps from "@/components/content-creation/techniques-steps";
import SubmissionSteps from "@/components/content-creation/submission-steps";
import UserHeader from "@/components/user-header";
import IfAuth from "@/components/auth/if-auth";

export default {
  name: "technique-info-card",
  components: {IfAuth, UserHeader, SubmissionSteps},
  props: {
    technique: {
      required: true,
      type: Object,
    },

    link: {
      required: false,
      type: Boolean,
      default: false
    },

    close: {
      required: false,
      type: Function,
      default: () => {
      }
    }
  },

  methods: {
    ...mapMutations("content-creation", ["activate"]),

    // Method for editing Technique
    edit() {
      // Invoke activate mutation from content-creation store
      // Passing TechniqueSteps as component, set edit as true, and editPayload as this technique => page data/local state
      this.activate({component: TechniqueSteps, editPayload: this.technique})
    },

    // Method for uploading Technique => Submission
    upload() {
      // Invoke activate mutation from content-creation store
      this.activate({
        component: SubmissionSteps,
        setup: (form) => form.techniqueId = this.technique.slug
      })
    }
  },

  // Map state for submissions and techniques, mapping getters for returning technique by id
  computed: {
    // Importing dictionary from initial state of techniques store => for particular technique we use dict => indexing
    ...mapState("library", ["dictionary"]),

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

    category() {
      return this.dictionary.categories[this.technique.category]
    },

    subcategory() {
      return this.dictionary.subcategories[this.technique.subCategory]
    },

  },
}
</script>

<style scoped>

</style>
