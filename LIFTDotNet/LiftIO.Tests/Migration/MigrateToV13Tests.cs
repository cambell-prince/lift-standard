﻿using LiftIO.Migration;
using LiftIO.Validation;
using NUnit.Framework;

namespace LiftIO.Tests.Migration
{
	[TestFixture]
	public class MigrateToV13Tests : MigratorTestBase
	{
		[Test]
		public void LiftVersion_Was0Point12_IsSetTo0Point13()
		{
			using (TempFile f = new TempFile("<lift version='0.12' producer='testing'/>"))
			{
				string path = Migrator.MigrateToLatestVersion(f.Path);
				Assert.AreEqual(Validator.LiftVersion, Validator.GetLiftVersion(path));
				AssertXPathAtLeastOne("//lift[@producer='testing']", path);
			}
		}

		[Test]
		public void SenseLiteralDefinition_WasOnSense_MovedToEntry()
		{
			using (TempFile f = new TempFile("<lift version='0.12' producer='tester'>" +
				"<entry>" +
				"<sense>" +
				"<field type='LiteralMeaning' dateCreated='2009-03-31T08:28:37Z'><form lang='en'><text>trial</text></form></field>" +
				"<trait name='SemanticDomainDdp4' value='6.1.2.9 Opportunity'/>" +
				"</sense>" +
				"</entry>" +
				"</lift>"))
			{
				string path = Migrator.MigrateToLatestVersion(f.Path);
				Assert.AreEqual(Validator.LiftVersion, Validator.GetLiftVersion(path));
				AssertXPathAtLeastOne("//lift[@producer='tester']", path);
				AssertXPathAtLeastOne("//entry/field[@type='literal-meaning']", path);
				AssertXPathNotFound("//entry/sense/field", path);
				AssertXPathAtLeastOne("//entry/sense/trait[@name='semantic-domain-ddp4']", path);
				AssertXPathNotFound("//entry/sense/trait[@name='SemanticDomainDdp4']", path);
			}
		}

		[Test]
		public void FileWas0Point11_EtymologyInSense_MovedToEntry()
		{
			using (TempFile f = new TempFile("<lift version='0.11' producer='testing'><entry><sense><etymology/></sense></entry></lift>"))
			{
				string path = Migrator.MigrateToLatestVersion(f.Path);
				using (TempFile.TrackExisting(path))
				{
					Assert.AreEqual(Validator.LiftVersion, Validator.GetLiftVersion(path));
					AssertXPathAtLeastOne("//lift[@producer='testing']", path);
					AssertXPathAtLeastOne("//entry/etymology", path);
				}
			}
		}

        [Test]
        public void TraitHasOldSkipFlag_ChangedToHyphenatedForm()
        {
            using (TempFile f = new TempFile(@"
                <lift version='0.12'>
                      <entry>
                        <trait name='flag_skip_FooBar' value='set' />
                      </entry>
                </lift>"))
            {
                string path = Migrator.MigrateToLatestVersion(f.Path);
                AssertXPathNotFound("//entry/trait[@name='flag_skip_FooBar']", path);
                AssertXPathAtLeastOne("//entry/trait[@name='flag-skip-FooBar']", path);
            }
        }
	}
}
