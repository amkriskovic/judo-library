﻿<template>
  <div>
    <!-- Check for modItem -->
    <div v-if="modItem">
      <!-- Display modItem's description -->
      {{modItem.description}}
    </div>

    <!-- Comment body -->
    <div>
      <v-text-field label="Comment" v-model="comment"></v-text-field>
      <v-btn @click="post">Post</v-btn>
    </div>

    <!-- Loop over comments, v-html will take care for rendering links/html tags etc -->
    <div v-for="comment in comments" v-html="comment.htmlContent"></div>
  </div>
</template>

<script>
  // Separate from component, easier to re-use it
  // Resolves endpoint based on type it's passed
  const endpointResolver = (type) => {
    // If type is technique, returns techniques string which we will use for resolving our API endpoint
    if (type === 'technique') return 'techniques'
  }

  export default {
    // Local state
    data: () => ({
      modItem: null,
      comments: [],
      comment: ""
    }),

    // Every time you need to get asynchronous data. fetch is called on server-side when rendering the route, and on client-side when navigating.
    created() {
      // We want extract data that we passed in from url params in /moderation/{}/{}/{}
      const {modItem, type, techniqueId} = this.$route.params

      // Produce the endpoint based on url type parameter, e.g. techniques
      const endpoint = endpointResolver(type)

      // Assign endpoint to the modItem in local state
      // Get dynamic API controller => response - data, based on URL parameters that we extracted
      // * Getting particular technique that we wanna moderate?
      this.$axios.$get(`/api/${endpoint}/${techniqueId}`)
        // Fire and forget
        // Then assign modItem(technique) that came from GET req. to local state item
        .then(modItem => this.modItem = modItem)

      // * Getting comments for particular moderation item
      this.$axios.$get(`/api/moderation-items/${modItem}/comments`)
        // Then assign comments that came from GET req. to local state comments arr
        .then(comments => this.comments = comments)
    },

    methods: {
      post() {
        // Extract moderation item id from URL param
        var {modItem} = this.$route.params;

        // Creating comment for particular moderation item, based on his Id that's passed via URL
        // As data to save to our API, pass object with content prop, which is comment "" from local state, v-model, binding
        this.$axios.$post(`/api/moderation-items/${modItem}/comments`, {content: this.comment})
          // Push created comment to comments arr
          .then(comment => this.comments.push(comment));
      }
    }

  }
</script>

<style scoped>

</style>
