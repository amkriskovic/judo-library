<template>
  <div>
    <!-- 1# Binding from comment-body component :comment prop to our comment prop, emitting send event -> both sending to same func -->
    <!-- Replaying to top comment, @load-replies event => loads replies -->
    <comment-body :comment="comment" @send="send" @load-replies="loadReplies"/>

    <!-- 2# -->
    <div class="ml-5">
      <!-- Replies part, pulling reply from replies, binding reply to comment prop, emitting send event -> both sending to same func -->
      <!-- Replying to one of repays -->
      <comment-body v-for="reply in replies" :comment="reply" @send="send"/>
    </div>
  </div>
</template>

<script>
  import CommentBody from "./comment-body";

  export default {
    // Component name
    name: "comment",

    // Injected components
    components: {CommentBody},

    // Component props => this is where comment info is stored
    props: {
      // Comment
      comment: {
        required: true,
        type: Object,
      }
    },

    // Component local state
    data: () => ({
      // Replies collection
      replies: [],
    }),

    // Component methods
    methods: {
      // Passing content from @send event -> creating reply for base comment
      send(content) {
        // Create reply for particular comment
        return this.$axios.$post(`/api/comments/${this.comment.id}/replies`, {content: content})
          // Push actual comment "reply" to arr of replies, we dont need to search for particular comment, we are already getting it
          // at this point, since we specified comment.id in route
          .then(reply => this.replies.push(reply))
      },

      // Load replies for specified comment
      loadReplies() {
        // comment.id is parent comment (base) for reply => comes from props
        return this.$axios.$get(`/api/comments/${this.comment.id}/replies`)
          // Assign replies that we got for specified comment to replies arr
          .then(replies => this.replies = replies)
      },
    }

  }
</script>

<style scoped>

</style>
