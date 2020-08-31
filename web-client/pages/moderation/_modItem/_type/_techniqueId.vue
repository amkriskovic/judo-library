﻿
<template>
  <div>
    <!-- Check for modItem -->
    <div v-if="modItem">
      <!-- Display modItem's description -->
      {{modItem.description}}
    </div>

    <!-- If we have parentCommentId -> means we have reply -->
    <div v-if="parentCommentId > 0">
      Replying to: {{parentCommentId}}
      <!-- On click, clear -->
      <v-btn @click="parentCommentId = 0">Clear</v-btn>
    </div>

    <!-- Comment body -->
    <div>
      <v-text-field label="Comment" v-model="comment"></v-text-field>
      <v-btn @click="post">Post</v-btn>
    </div>

    <!-- Loop over comments, v-html will take care for rendering links/html tags etc -->
    <div class="my-1" v-for="comment in comments">
      <span v-html="comment.htmlContent"></span>
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
      post() {
        // Extract moderation item id from URL param
        const {modItem} = this.$route.params;

        // If we have parentCommentId means we can create reply for it
        if (this.parentCommentId > 0) {

          // Create reply for particular comment
          this.$axios.$post(`/api/comments/${this.parentCommentId}/replies`)
            .then(reply => this.comments
              // Find the actual comment that we are replying to -> get particular comment => parent comment
              .find(c => c.id === this.parentCommentId)
              // Pipe replies, push to replies that reply from form body
              .replies.push(reply))
        } else {
          // If it's not reply, it's regular comment
          // Creating comment for particular moderation item, based on his Id that's passed via URL
          // As data to save to our API, pass object with content prop, which is comment "" from local state, v-model, binding
          this.$axios.$post(`/api/moderation-items/${modItem}/comments`, {content: this.comment})
            // Push created comment to commentWithReplies func that will take comment and spread it into object
            .then(comment => this.comments.push(commentWithReplies(comment)));
        }

      },

      // Load replies for specified comment
      loadReplies(comment) {
        this.$axios.$get(`/api/comments/${comment.id}/replies`)
          // Then set comment to replies from comments that are coming from API response
          // $set => What do we wanna set, what prop we wanna set, what do we want to set it to
          .then(comments => this.$set(comment, 'replies', comments))
      }

    }

  }
</script>

<style scoped>

</style>
