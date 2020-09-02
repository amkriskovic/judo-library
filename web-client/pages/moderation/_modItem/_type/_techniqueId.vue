﻿
<template>
  <div>
    <!-- Check for modItem -->
    <div v-if="modItem">
      <!-- Display modItem's description -->
      {{modItem.description}}
    </div>

    <!-- Injecting comment section component, dyn binding comments so we can display them, hooking send event -->
    <!-- * This is the place where moderator creates comment, this is creation of base comment -->
    <!-- :comments is coming from comment-section prop -->
    <comment-section :comments="comments" @send="send"/>

    <!-- Loop over comments, v-html will take care for rendering links/html tags etc -->
    <div class="my-1" v-for="comment in comments">
      <!-- On click, set that base comment id to parentCommentId, so that we know which reply belong to what comment -->
      <v-btn small @click="parentCommentId = comment.id">Reply</v-btn>

      <!-- On click load replies based on comment that we click -> passing that comment -->
      <v-btn small @click="loadReplies(comment)">Load Reply</v-btn>

      <!-- Grabbing replies from comment object -->
      <div v-for="reply in comment.replies">
        <!-- Display html content of reply -->
        <span v-html="reply.htmlContent"></span>
      </div>
    </div>

  </div>
</template>

<script>
  // Separate from component, easier to re-use it
  // Resolves endpoint based on type it's passed
  import CommentSection from "../../../../components/comments/comment-section";

  const endpointResolver = (type) => {
    // If type is technique, returns techniques string which we will use for resolving our API endpoint
    if (type === 'technique') return 'techniques'
  }

  // Func that takes comment as input and spreads that comment into object, with addition of replies arr
  const commentWithReplies = (comment) => ({
    // Grab all the props comment has
    ...comment,

    // Add new custom prop to this object, replies arr so we can push replies to it
    // Only parent comments gonna have replies array
    replies: []
  })

  export default {
    components: {CommentSection},
    // Local state
    data: () => ({
      modItem: null,
      comments: [],
      comment: "",
      parentCommentId: 0
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
        // Map comments into function that returns custom object, it will populate object with response of comments
        // and create empty arr of replies for start
        .then(comments => this.comments = comments.map(commentWithReplies))
    },

    methods: {
      send(content) {
        // Extract moderation item id from URL param
        const {modItem} = this.$route.params;

        // If it's not reply, it's regular comment
        // Returning promise | Creating comment for particular moderation item, based on his Id that's passed via URL
        // As data to save to our API, pass object with content prop, which is comment "" from local state, v-model, binding
        return this.$axios.$post(`/api/moderation-items/${modItem}/comments`, {content: content})
          // Then push comment to local state arr of comments
          .then(comment => this.comments.push(comment));
      }

    },

  }
</script>

<style scoped>

</style>
