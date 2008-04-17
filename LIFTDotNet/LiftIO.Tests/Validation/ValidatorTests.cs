using System;
using System.IO;
using System.Xml;
using LiftIO.Parsing;
using LiftIO.Validation;
using NUnit.Framework;

namespace LiftIO.Tests.Validation
{
    [TestFixture]
    public class ValidatorTests
    {
      
        [Test]
        public void LiftVersion_MatchesEmbeddedSchemaVersion()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(typeof (LiftMultiText).Assembly.GetManifestResourceStream("LiftIO.Validation.lift.rng"));
            string query = String.Format("//x:attribute/x:value[.='{0}']", Validator.LiftVersion);
            XmlNamespaceManager m = new XmlNamespaceManager(doc.NameTable);
            m.AddNamespace("x", "http://relaxng.org/ns/structure/1.0");
            Assert.IsNotNull(doc.FirstChild.SelectSingleNode(query, m));
        }

        [Test]
        public void Validate_EmptyFile_Validates()
        {
            string contents = string.Format("<lift version='{0}'></lift>", Validator.LiftVersion);
            Validate(contents, true);
        }

        [Test]
        public void Validate_BadLift_DoesNotValidate()
        {
            string contents = "<lift version='0.10'><header></header><header></header></lift>";
            Validate(contents, false);
        }

        [Test]
        public void WrongVersionNumberGivesHelpfulMessage()
        {
            string contents = "<lift version='0.8'><header></header><header></header></lift>";
            string errors = Validate(contents, false);
            Assert.IsTrue(errors.Contains("This file claims to be version"));
        }

        private static string Validate(string contents, bool shouldPass)
        {
            string f = Path.GetTempFileName();
            File.WriteAllText(f, contents);
            string errors;
            try
            {
                errors = Validator.GetAnyValidationErrors(f);
                if(shouldPass)
                {
                    if (errors != null)
                    {
                        Console.WriteLine(errors);
                    }
                    Assert.IsNull(errors);
                }
                else
                {
                    Assert.IsNotNull(errors);
                }
            }
            finally
            {
                File.Delete(f);
            }
            return errors;
        }

    }
}