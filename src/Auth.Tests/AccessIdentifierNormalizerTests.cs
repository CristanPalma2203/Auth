using Domain.Helpers;
using NUnit.Framework;

namespace Auth.Tests
{
    public class AccessIdentifierNormalizerTests
    {
        [Test]
        public void Normalize_keeps_plus_in_json_email()
        {
            Assert.That(
                AccessIdentifierNormalizer.Normalize("qa.smoke+stg@luxware.co"),
                Is.EqualTo("qa.smoke+stg@luxware.co"));
        }

        [Test]
        public void Normalize_restores_form_urlencoded_plus_as_space()
        {
            Assert.That(
                AccessIdentifierNormalizer.Normalize("qa.smoke stg@luxware.co"),
                Is.EqualTo("qa.smoke+stg@luxware.co"));
        }

        [Test]
        public void Normalize_leaves_document_ids_unchanged()
        {
            Assert.That(AccessIdentifierNormalizer.Normalize("admin"), Is.EqualTo("admin"));
            Assert.That(AccessIdentifierNormalizer.Normalize("01234567-8"), Is.EqualTo("01234567-8"));
        }
    }
}
