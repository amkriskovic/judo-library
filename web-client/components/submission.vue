<template>
  <v-card :elevation="elevation" :class="{'highlight': highlight}">
    <!-- Injecting user-header component, providing username from submission -->
    <user-header class="pa-2"
                 :username="submission.user.username"
                 :image-url="submission.user.image"
                 :size="slim ? '32' : '42'">
      <template v-slot:append>
        <div v-if="slim">
          <v-icon>mdi-chevron-up</v-icon>
          <span>{{ submission.score }}</span>
          <v-icon>mdi-chevron-down</v-icon>
        </div>
        <span v-else class="caption grey--text">{{ submission.created }}</span>
      </template>
    </user-header>

    <v-card-text v-if="!slim">{{ submission.description }}</v-card-text>

    <video-player :video="submission.video" :thumb="submission.thumb"/>

    <if-auth v-if="!slim">
      <template v-slot:allowed>
        <v-card-actions>
          <v-btn icon :color="submission.vote === 1 ? 'blue' : ''" @click="vote(1)">
            <v-icon>mdi-chevron-up</v-icon>
          </v-btn>
          <span class="mx-2">{{ submission.score }}</span>
          <v-btn icon :color="submission.vote === -1 ? 'blue' : ''" @click="vote(-1)">
            <v-icon>mdi-chevron-down</v-icon>
          </v-btn>

          <v-spacer/>

          <v-btn icon @click="showComments = !showComments">
            <v-icon>mdi-comment</v-icon>
          </v-btn>
        </v-card-actions>
        <comment-section v-if="showComments" class="px-3" :parent-id="submission.id"
                         :parent-type="submissionParentType"/>
      </template>

      <template v-slot:forbidden="{login}">
        <v-card-actions>
          <v-btn icon disabled>
            <v-icon>mdi-chevron-up</v-icon>
          </v-btn>
          <span class="mx-2">{{ submission.score }}</span>
          <v-btn icon disabled>
            <v-icon>mdi-chevron-down</v-icon>
          </v-btn>

          <v-spacer/>

          <v-btn small @click="login">Sign in to Comment</v-btn>
        </v-card-actions>
      </template>
    </if-auth>

  </v-card>
</template>

<script>
import UserHeader from "@/components/user-header";
import VideoPlayer from "@/components/video-player";
import {COMMENT_PARENT_TYPE} from "@/components/comments/_shared";
import CommentSection from "@/components/comments/comment-section";
import IfAuth from "@/components/auth/if-auth";

export default {
  name: "submission",
  components: {IfAuth, CommentSection, VideoPlayer, UserHeader},

  // Defining that submission needs to be passed as an object
  props: {
    submission: {
      type: Object,
      required: true,
    },

     slim: {
      type: Boolean,
      required: false,
      default: false,
    },

    elevation: {
      type: String,
      required: false,
      default: '8',
    }
  },

  data: () => ({
    showComments: false
  }),

  computed: {
    submissionParentType() {
      return COMMENT_PARENT_TYPE.SUBMISSION
    },

    highlight() {
      return (this.$route.query.submission | 0) === this.submission.id
    },
  },

  methods: {
    vote(value) {
      if (this.submission.vote === value) return;

      // If it's the 1st time we are voting, add the value (+1), otherwise +2
      this.submission.score += this.submission.vote === 0 ? value : value * 2

      this.submission.vote = value

      return this.$axios.$put(`/api/submissions/${this.submission.id}/vote?value=${value}`, null)
    }
  }

}
</script>

<style scoped>
 .highlight {
   box-shadow: 0 1px 8px 0 #1976d2 !important;
 }
</style>
