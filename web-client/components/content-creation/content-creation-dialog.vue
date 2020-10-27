<template>
  <!-- Dialog - Popup - based on state(value) of active prop in video-upload store state -->
  <v-dialog :value="active" persistent width="700">

    <!-- Activator will activate the component when clicked -->
    <template v-slot:activator="{on}">
      <!-- Menu component - Dropdown -->
      <v-menu offset-y>
        <template v-slot:activator="{ on, attrs }">
          <!-- Button -->
          <v-btn depressed v-bind="attrs" v-on="on">
            Create
          </v-btn>
        </template>

        <!-- Dropdown content -->
        <v-list>
          <!-- # Iterating over components -->
          <!-- {component name}-menu-index, on click activate component that we chose -->
          <!-- * deconstruct / sendComment component as payload and ACTIVATES (Component) it -->
          <v-list-item v-for="(modItem, index) in menuItems" :key="`ccd-menu-${index}`"
                       @click="activate({component: modItem.component})">
            <v-list-item-title>{{ modItem.title }}</v-list-item-title>
          </v-list-item>
        </v-list>
      </v-menu>
    </template>

    <!-- If component exists, display it (insert it here) -->
    <div v-if="component">
      <!-- # Place for new components -->
      <!-- This component value is gonna come from vuex store, dynamically based on what component we choose -->
      <component :is="component"></component>
    </div>

  </v-dialog>
</template>

<script>
import {mapState, mapMutations, mapActions, mapGetters} from "vuex";
  import TechniquesSteps from "./techniques-steps";
  import SubmissionSteps from "./submission-steps";
  import CategoryForm from "./category-form";
  import SubcategoryForm from "./subcategory-form";

  export default {
    // Component name
    name: "content-creation-dialog",

    // List of components
    components: {TechniquesSteps, SubmissionSteps, CategoryForm, SubcategoryForm},

    // Mapping state for video-upload store | computed === state
    computed: {
      ...mapState("video-upload", ["active", "component"]),
      ...mapGetters("auth", ["moderator"]),


      // Binding out components to menu items
      menuItems() {
        return [
          // Returns array of objects, Affects component order,
          // Only Mod's will be able to create Category,Sub-Category of Technique
          {component: TechniquesSteps, title: "Technique", display: true},
          {component: SubmissionSteps, title: "Submission", display: true},
          {component: CategoryForm, title: "Category", display: this.moderator},
          {component: SubcategoryForm, title: "Sub-Category", display: this.moderator},
        ].filter(x => x.display)
      }
    },

    // Mapping mutations & actions for video-upload | methods === functions ==> mutations/actions
    methods: mapMutations("video-upload", ["activate"])

  }
</script>

<style scoped>

</style>
