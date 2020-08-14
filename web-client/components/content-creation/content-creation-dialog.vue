﻿<template>
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
          <!-- * deconstruct / send component as payload and ACTIVATES (Component) it -->
          <v-list-item v-for="(item, index) in menuItems" :key="`ccd-menu-${index}`"
                       @click="activate({component: item.component})">
            <v-list-item-title>{{ item.title }}</v-list-item-title>
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

    <!-- Button -->
    <!-- Centered based on flex, margin top and bottom - 4  -->
    <!-- Cancel form on close -> cancel upload -> delete upload video if it was saved -->
    <div class="d-flex justify-center my-4">
      <v-btn @click="cancelUpload">
        Close
      </v-btn>
    </div>

  </v-dialog>
</template>

<script>
  import {mapState, mapMutations, mapActions} from "vuex";
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

      // Binding out components to menu items
      menuItems() {
        return [
          // Returns array of objects, Affects component order
          {component: TechniquesSteps, title: "Technique"},
          {component: SubmissionSteps, title: "Submission"},
          {component: CategoryForm, title: "Category"},
          {component: SubcategoryForm, title: "Sub-Category"},
        ]
      }
    },

    // Mapping mutations & actions for video-upload | methods === functions ==> mutations/actions
    methods: {
      ...mapMutations("video-upload", ["reset", "activate"]),
      ...mapActions("video-upload", ["cancelUpload"])
    }

  }
</script>

<style scoped>

</style>
