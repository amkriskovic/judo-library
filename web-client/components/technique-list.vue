<template>
  <div>
    <!-- Input text field, mdi -> material design icons - {name of icon}, two way binding -> filter -->
    <v-text-field label="Search" placeholder="e.g. throw/reap/drop" v-model="filter" prepend-inner-icon="mdi-magnify"
                  outlined>

    </v-text-field>
    <!-- Iterating over filtered Techniques -->
    <div v-for="technique in filteredTechniques">
      {{ technique.name }} - {{ technique.description }}
    </div>
  </div>
</template>

<script>
  // Component for displaying techniques and filtering based on typed technique name or description

  export default {
    // Component name
    name: "technique-list",

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
      // Function that returns filtered techniques based on search | Mutating page state | Interacting with page data
      filteredTechniques() {
        // If there is no filter(search) applied, just return original tricks
        if (!this.filter) return this.techniques;

        // Normalize/Sanitize filter(search) input/string
        const normalized = this.filter.trim().toLowerCase();

        // * Returning filtered techniques array -> name and description,
        // to lower both, passing normalized string that came from search input (v-model)
        return this.techniques.filter(t => t.name.toLowerCase().includes(normalized) || t.description.toLowerCase().includes(normalized));
      }
    }
  }
</script>

<style scoped>

</style>
