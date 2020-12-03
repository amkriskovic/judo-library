<template>
  <div>
    <!-- Input text field, mdi -> material design icons - {name of icon}, two way binding -> filter -->
    <v-text-field label="Search" placeholder="e.g. throw/reap/drop" v-model="filter" prepend-inner-icon="mdi-magnify"
                  outlined>
    </v-text-field>

    <v-row justify="center">
      <v-col class="d-flex justify-center align-start"
             lg="3"
             v-for="technique in filteredTechniques" :key="`technique-info-card-${technique.id}`">
        <technique-info-card :technique="technique"/>
      </v-col>
    </v-row>
  </div>
</template>

<script>
  // Component for displaying techniques and filtering based on typed technique name or description

  import TechniqueInfoCard from "@/components/technique-info-card";
  import {hasOccurrences} from "@/data/functions";
  export default {
    // Component name
    name: "technique-list",
    components: {TechniqueInfoCard},
    // Component local state
    data: () => ({
      filter: ""
    }),

    // Defining props object which contains "arguments"
    props: {
      // Defining techniques as argument (object)
      techniques: {
        required: true, // Mark as required, otherwise component won't work
        type: Array, // Marking as type Array( [] )
      }
    },

    computed: {
      filteredTechniques() {
        if (!this.filter) return this.techniques;

        return this.techniques.filter(t => {
          let searchIndex = (t.name + t.description).toLowerCase()
          return hasOccurrences(searchIndex, this.filter)
        })

      }
    }
  }
</script>

<style scoped>

</style>
