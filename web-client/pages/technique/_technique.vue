<template>
  <div class="d-flex justify-center align-start">

    <div class="mx-2" v-if="submissions">
      <div v-for="submission in 1">
        <div v-for="submission in submissions">
          {{ submission.id }} - {{ submission.description }} - {{ submission.techniqueId }}
          <div>
            <!-- :src is calling our /api/videos/{video} controller to open and read content from it -->
            <video width="400" controls :src="`http://localhost:5000/api/videos/${ submission.video }`"></video>
          </div>
        </div>
      </div>
    </div>

    <!-- Component for http://localhost:3000/technique/{everything that comes after that (slug)} -->
    <!-- v-sheet represents "card" with information -->
    <v-sheet class="pa-3 ma-2 sticky">
      <div class="text-h5">
        <span>{{ technique.name }}</span>
        <!-- :to= corresponds to page that we created | category == folder | category.id == page?  -->
        <v-chip small label class="mb-1 ml-2" :to="`/category/${category.id}`">Category: {{ category.name }}</v-chip>
        <v-chip small label class="mb-1 ml-2" :to="`/subcategory/${subcategory.id}`">Sub Category: {{ subcategory.name }}</v-chip>
      </div>

      <v-divider class="my-1"></v-divider>
      <div class="text-body-2"> {{ technique.description }}</div>
      <v-divider class="my-1"></v-divider>

      <!-- Auto-generated component based what relatedData func is returning -->
      <div v-for="rd in relatedData" v-if="rd.data.length > 0"> <!-- * If there is data, only then display whole object inforation -->
        <div class="text-subtitle-1">{{ rd.title }}</div>

        <v-chip-group>
          <!-- If something is in the same category, component get's re-rendered (cached), d is data: which is technique(id) for now -->
          <v-chip v-for="d in rd.data" :key="rd.idFactory(d)" :to="rd.routeFactory(d)" small label>
            {{ d.name }}
          </v-chip>
        </v-chip-group>
      </div>
    </v-sheet>

  </div>
</template>

<script>
  import {mapState, mapGetters} from "vuex";

  export default {
    // Page local state
    data: () => ({
      technique: null,
      category: null,
      subcategory: null,
    }),

    // Map state for submissions and techniques, mapping getters for returning technique by id
    computed: {
      ...mapState("submissions", ["submissions"]),
      ...mapState("techniques", ["techniques"]),

      ...mapGetters("techniques", ["techniqueById", "categoryById", "subcategoryById"]),

      // Function that returns object with title, data -> for specific technique, related data in this case filters:
      // subcategory, setup attacks and followup attacks, idFactory for uniqueness and routeFactory for navigating
      // thorough chips to respective technique
      relatedData() {
        return [
          {
            title: "Set Up Attacks",
            data: this.techniques.filter(t => this.technique.followUpAttacks.indexOf(t.id) >= 0),
            idFactory: t => `technique-${t.id}`,
            routeFactory: t => `/technique/${t.id}`,
          },
          {
            title: "Follow Up Attacks",
            data: this.techniques.filter(t => this.technique.setUpAttacks.indexOf(t.id) >= 0),
            idFactory: t => `technique-${t.id}`,
            routeFactory: t => `/technique/${t.id}`,
          },
          {
            title: "Counters",
            data: this.techniques.filter(t => this.technique.counters.indexOf(t.id) >= 0),
            idFactory: t => `technique-${t.id}`,
            routeFactory: t => `/technique/${t.id}`
          }
        ]
      },

    },

    // Pre-fetching data asynchronously for this particular page
    async fetch() {
      // Getting techniqueId from URL param
      const techniqueId = this.$route.params.technique;

      // Assigning particular grabbed technique from url -> id to pages local state technique
      this.technique = this.techniqueById(this.$route.params.technique);

      // Assign category(which we get based on technique above | grabbing from technique) and assign to local state category
      this.category = this.categoryById(this.technique.category);

      // Assign subcategory(which we get based on technique above | grabbing from technique) and assign to local state subCategory
      this.subcategory = this.subcategoryById(this.technique.subCategory);

      // dispatching fetchSubmissionsForTechnique action from submissions store, passing techniqueId as argument
      await this.$store.dispatch("submissions/fetchSubmissionsForTechnique", {techniqueId}, {root: true}); // Dispatch action as root
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
