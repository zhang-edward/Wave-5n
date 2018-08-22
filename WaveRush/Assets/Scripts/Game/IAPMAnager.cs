// using UnityEngine;
// using UnityEngine.Purchasing;

// public class IAPManager : IStoreListener {

// 	// private static string REMOVE_ADS_ID = "REMOVE_ADS";
// 	// private static string REMOVE_ADS_ID_APPLE = "REMOVE_ADS_APPLE";


// 	private IStoreController controller;
// 	private IExtensionProvider extensions;

// 	void Start() {
// 		// If we haven't set up the Unity Purchasing reference
// 		if (controller == null) {
// 			// Begin to configure our connection to Purchasing
// 			InitializePurchasing();
// 		}
// 	}
	
// 	public void InitializePurchasing() {
// 		// If we have already connected to Purchasing ...
// 		if (IsInitialized()) {
// 			// ... we are done here.
// 			return;
// 		}
		
// 		// Create a builder, first passing in a suite of Unity provided stores.
// 		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		
// 		// // Add a product to sell / restore by way of its identifier, associating the general identifier
// 		// // with its store-specific identifiers.
// 		// builder.AddProduct(kProductIDConsumable, ProductType.Consumable);
// 		// // Continue adding the non-consumable product.
// 		// builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);
// 		// // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
// 		// // if the Product ID was configured differently between Apple and Google stores. Also note that
// 		// // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
// 		// // must only be referenced here. 
// 		// builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
// 		// 	{ kProductNameAppleSubscription, AppleAppStore.Name },
// 		// 	{ kProductNameGooglePlaySubscription, GooglePlay.Name },
// 		// });
		
// 		// Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
// 		// and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
// 		UnityPurchasing.Initialize(this, builder);
// 	}

// 	private bool IsInitialized()
// 	{
// 		// Only say we are initialized if both the Purchasing references are set.
// 		return controller != null && extensions != null;
// 	}

// 	/// <summary>
//     /// Called when Unity IAP is ready to make purchases.
//     /// </summary>
//     public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
//     {
//         this.controller = controller;
//         this.extensions = extensions;
//     }

// }