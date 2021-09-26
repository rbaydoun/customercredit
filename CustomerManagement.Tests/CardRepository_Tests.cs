using System;
using CustomerManagement.DAL;
using CustomerManagement.Encryption;
using CustomerManagement.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CustomerManagement.Tests
{
  public class CardRepository_Tests
  {

    // Skipping more trivial tests, testing that encryption/descryption works.
    [Fact]
    public void Insert_EncryptedDataReplaced()
    {
      // Arrange
      string unencryptedNumber = "Number";
      string unencryptedCvv = "CVV";
      Card card = new Card() { Number = unencryptedNumber, Cvv = unencryptedCvv };

      var dbContextMock = new Mock<CustomerManagementContext>();
      var dbSetCardMock = new Mock<DbSet<Card>>();
      var protectorMock = new Mock<IDataProtector>();
      var protectionProviderMock = new Mock<IDataProtectionProvider>();

      protectionProviderMock.Setup(x => x.CreateProtector(DataProtectionPurposeStrings.CreditCardInformation)).Returns(protectorMock.Object);
      // Apparently cannot use these extension methods for setup or verification.
      //protectorMock.Setup(x => x.Protect(unencryptedNumber)).Returns(encryptedNumber);
      //protectorMock.Setup(x => x.Protect(unencryptedCvv)).Returns(encryptedCvv);
      dbContextMock.Setup(x => x.Cards).Returns(dbSetCardMock.Object);

      var cardRepository = new CardRepository(dbContextMock.Object, protectionProviderMock.Object);

      // Act
      cardRepository.Insert(card);

      // Assert
      Assert.NotEqual(unencryptedNumber, card.Number);
      Assert.NotEqual(unencryptedCvv, card.Cvv);
    }

    [Fact]
    public void GetById_EncryptedDataReplaced()
    {
      // Arrange
      string encryptedNumber = "EncryptedNumber";
      string encryptedCvv = "EncryptedCVV";
      long id = 1;
      Card card = new Card() { Number = encryptedNumber, Cvv = encryptedCvv };

      var dbContextMock = new Mock<CustomerManagementContext>();
      var dbSetCardMock = new Mock<DbSet<Card>>();
      var protectorMock = new Mock<IDataProtector>();
      var protectionProviderMock = new Mock<IDataProtectionProvider>();

      protectionProviderMock.Setup(x => x.CreateProtector(DataProtectionPurposeStrings.CreditCardInformation)).Returns(protectorMock.Object);
      // Apparently cannot use these extension methods for setup or verification.
      //protectorMock.Setup(x => x.Protect(unencryptedNumber)).Returns(encryptedNumber);
      //protectorMock.Setup(x => x.Protect(unencryptedCvv)).Returns(encryptedCvv);
      dbContextMock.Setup(x => x.Cards).Returns(dbSetCardMock.Object);
      dbSetCardMock.Setup(x => x.Find(id)).Returns(card);

      var cardRepository = new CardRepository(dbContextMock.Object, protectionProviderMock.Object);

      // Act
      var result = cardRepository.GetCardById(id);

      // Assert
      Assert.NotEqual(encryptedNumber, result.Number);
      Assert.NotEqual(encryptedCvv, result.Cvv);
    }
  }
}
