import {mapState} from "vuex";

export default {
  methods: {
    gotoSubmission(techniqueSlug, submissionId) {
      this.$router.push(`/technique/${techniqueSlug}?submission=${submissionId}`)
    }
  },
  computed: {
    ...mapState('techniques', ['lists','dictionary']),
    completedTechniques() {
      const submissions = this.profile.submissions
        .filter((v, i, a) => a
          .map(x => x.techniqueId)
          .indexOf(v.techniqueId) === i);

      return submissions.map(submission => ({
        submission,
        technique: this.dictionary.techniques[submission.techniqueId]
      }))
    }
  }
}
