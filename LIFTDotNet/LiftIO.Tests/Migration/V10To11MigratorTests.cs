using LiftIO.Migration;
using LiftIO.Validation;
using NUnit.Framework;

namespace LiftIO.Tests.Migration
{
    [TestFixture]
    public class V10To11MigratorTests : MigratorTestBase
    {
        [Test]
        public void IsMigrationNeeded_ReturnsTrue()
        {
            using (TempFile f = new TempFile("<lift version='0.10'></lift>"))
            {
                Assert.IsTrue(Migrator.IsMigrationNeeded(f.Path));
            }
        }

        [Test]
        public void MigrateToLatestVersion_ConvertedToLatest()
        {
            using (TempFile f = new TempFile("<lift version='0.10'></lift>"))
            {
                string path = Migrator.MigrateToLatestVersion(f.Path);
                Assert.AreEqual(Validator.LiftVersion, Validator.GetLiftVersion(path));
            }
        }

        [Test]
        public void RelationHasNameAttribute_ChangedToType()
        {
            using (TempFile f = new TempFile("<lift version='0.10'><entry><relation order='2' ref='xyz' name='foo'/></entry></lift>"))
            {
                string path = Migrator.MigrateToLatestVersion(f.Path);
                using (TempFile.TrackExisting(path))
                {
                    AssertXPathAtLeastOne("//relation[@order='2' and @ref='xyz' and @type='foo']", path);
                    AssertXPathNotFound("//relation/@name", path);
                }
            }
        }

        [Test]
        public void HasPicture_ChangedToIllustration()
        {
            using (TempFile f = new TempFile(@"
                <lift version='0.10'>
                      <entry>
                        <sense>
                          <illustration href='waterBasket1.png'/>
                        </sense>
                      </entry>
                </lift>"))
            {
                string path = Migrator.MigrateToLatestVersion(f.Path);
                AssertXPathAtLeastOne("//sense/illustration[@href='waterBasket1.png']", path);
                AssertXPathNotFound("//sense/picture", path);
            }
        }

        [Test]
        public void GlossHasTrait_ChangedToAnnotation()
        {
            using (TempFile f = new TempFile(@"
                <lift version='0.10'>
                      <entry>
                        <sense>
                          <gloss lang='en'>
                            <text>water carrying basket</text>
                            <trait name='flag' value='1' />
                          </gloss>
                        </sense>
                      </entry>
                </lift>"))
            {
                string path = Migrator.MigrateToLatestVersion(f.Path);
                AssertXPathAtLeastOne("//entry/sense/gloss/annotation[@value='1']", path);
                AssertXPathNotFound("//entry/sense/gloss/trait", path);
            }
        }

   
        [Test]
        public void FormHasTrait_ChangedToAnnotation()
        {
            using (TempFile f = new TempFile(@"
                <lift version='0.10'>
                      <entry>
                        <lexical-unit>
                          <form lang='bth'>
                            <text>abit</text>
                             <trait name='flag' value='1' />
                         </form>
                        </lexical-unit>
                      </entry>
                </lift>"))
            {
                string path = Migrator.MigrateToLatestVersion(f.Path);
                AssertXPathAtLeastOne("//entry/lexical-unit/form/annotation[@value='1']", path);
                AssertXPathNotFound("//entry/lexical-unit/form/trait", path);
            }
        }

        [Test]
        public void FieldHasTag_ChangedToType()
        {
            using (TempFile f = new TempFile(@"
                <lift version='0.10'>
                      <entry>
                        <field tag='test'/>
                      </entry>
                </lift>"))
            {
                string path = Migrator.MigrateToLatestVersion(f.Path);
                AssertXPathAtLeastOne("//entry/field[@type='test']", path);
                AssertXPathNotFound("//entry/field[@tag='test']", path);
            }
        }


        [Test]
        public void PreservesProducer()
        {
            using (TempFile f = new TempFile("<lift version='0.10' producer='p'/>"))
            {
                string path = Migrator.MigrateToLatestVersion(f.Path);
                AssertXPathAtLeastOne("//lift[@producer='p']", path);
            }
        }
    }
}